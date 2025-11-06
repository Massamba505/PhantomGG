import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '@/app/api/services';
import { ToastService } from '@/app/shared/services/toast.service';
import { passwordStrengthValidator } from '@/app/shared/validators/password.validator';
import { matchPasswordsValidator } from '@/app/shared/validators/match-passwords.validator';
import { getPasswordScore } from '@/app/shared/utils/PasswordScore';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-reset-password',
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
  templateUrl: './reset-password.html',
})
export class ResetPassword implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private toastService = inject(ToastService);

  readonly icons = LucideIcons;

  loading = signal(false);
  success = signal(false);
  error = signal('');
  token = signal('');

  resetPasswordForm = this.fb.group(
    {
      password: ['', [Validators.required, passwordStrengthValidator]],
      confirmPassword: ['', Validators.required],
    },
    { validators: matchPasswordsValidator }
  );

  ngOnInit() {
    const tokenParam = this.route.snapshot.queryParams['token'];

    if (!tokenParam) {
      this.error.set('Reset token is missing');
      return;
    }

    this.token.set(tokenParam);
  }

  get passwordStrength() {
    const value = this.resetPasswordForm.controls['password'].value || '';
    const score = getPasswordScore(value);
    if (!value) return { label: '', color: '' };
    if (score >= 5) return { label: 'Strong Password', color: 'text-success' };
    if (score >= 3) return { label: 'Medium Strength', color: 'text-warning' };
    return { label: 'Weak Password', color: 'text-destructive' };
  }

  checkFieldError(field: string): boolean {
    const control = this.resetPasswordForm.get(field);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  checkFormError(error: string): boolean {
    const form = this.resetPasswordForm;
    return !!(form.hasError(error) && form.touched);
  }

  onSubmit() {
    if (this.resetPasswordForm.invalid) {
      this.resetPasswordForm.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    const { password } = this.resetPasswordForm.value;

    this.authService
      .resetPassword({
        token: this.token(),
        newPassword: password!,
      })
      .subscribe({
        next: () => {
          this.loading.set(false);
          this.success.set(true);
          this.toastService.success(
            'Password reset successfully! You can now log in.'
          );
          setTimeout(() => {
            this.router.navigate(['/auth/login']);
          }, 3000);
        },
        error: (err) => {
          this.loading.set(false);
        },
      });
  }

  goToLogin() {
    this.router.navigate(['/auth/login']);
  }
}
