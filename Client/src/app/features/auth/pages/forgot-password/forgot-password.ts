import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '@/app/api/services';
import { ToastService } from '@/app/shared/services/toast.service';
import { strictEmailValidator } from '@/app/shared/validators/email.validator';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-forgot-password',
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
  templateUrl: './forgot-password.html',
})
export class ForgotPassword {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly toastService = inject(ToastService);

  readonly icons = LucideIcons;

  loading = signal(false);
  emailSent = signal(false);

  forgotPasswordForm = this.fb.group({
    email: ['', [Validators.required, strictEmailValidator]],
  });

  get email() {
    return this.forgotPasswordForm.get('email');
  }

  checkFieldError(field: string): boolean {
    const control = this.forgotPasswordForm.get(field);
    return !!(control && control.invalid && (control.dirty || control.touched));
  }

  onSubmit() {
    if (this.forgotPasswordForm.invalid) {
      this.forgotPasswordForm.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    const { email } = this.forgotPasswordForm.value;

    this.authService.forgotPassword({ email: email! }).subscribe({
      next: () => {
        this.loading.set(false);
        this.emailSent.set(true);
        this.toastService.success(
          'Password reset instructions sent to your email'
        );
      },
      error: (err) => {
        this.loading.set(false);
      },
    });
  }

  goToLogin() {
    this.router.navigate(['/auth/login']);
  }

  resendEmail() {
    this.emailSent.set(false);
    this.onSubmit();
  }
}
