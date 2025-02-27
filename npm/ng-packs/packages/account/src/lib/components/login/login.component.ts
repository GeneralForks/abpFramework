import { AuthService, ConfigStateService } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { Component, Injector, OnInit, inject } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { throwError } from 'rxjs';
import { catchError, finalize } from 'rxjs/operators';
import { eAccountComponents } from '../../enums/components';
import { getRedirectUrl } from '../../utils/auth-utils';

const { maxLength, required } = Validators;

@Component({
  standalone: false,
  selector: 'abp-login',
  templateUrl: './login.component.html',
})
export class LoginComponent implements OnInit {
  protected injector = inject(Injector);
  protected fb = inject(UntypedFormBuilder);
  protected toasterService = inject(ToasterService);
  protected authService = inject(AuthService);
  protected configState = inject(ConfigStateService);

  form!: UntypedFormGroup;

  inProgress?: boolean;

  isSelfRegistrationEnabled = true;

  authWrapperKey = eAccountComponents.AuthWrapper;

  ngOnInit() {
    this.init();
    this.buildForm();
  }

  protected init() {
    this.isSelfRegistrationEnabled =
      (
        (this.configState.getSetting('Abp.Account.IsSelfRegistrationEnabled') as string) || ''
      ).toLowerCase() !== 'false';
  }

  protected buildForm() {
    this.form = this.fb.group({
      username: ['', [required, maxLength(255)]],
      password: ['', [required, maxLength(128)]],
      rememberMe: [false],
    });
  }

  onSubmit() {
    if (this.form.invalid) return;

    this.inProgress = true;

    const { username, password, rememberMe } = this.form.value;

    const redirectUrl = getRedirectUrl(this.injector);

    this.authService
      .login({ username, password, rememberMe, redirectUrl })
      .pipe(
        catchError(err => {
          this.toasterService.error(
            err.error?.error_description ||
              err.error?.error.message ||
              'AbpAccount::DefaultErrorMessage',
            '',
            { life: 7000 },
          );
          return throwError(err);
        }),
        finalize(() => (this.inProgress = false)),
      )
      .subscribe();
  }
}
