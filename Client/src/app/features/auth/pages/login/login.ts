import { Component, inject, signal } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { strictEmailValidator } from '@/app/shared/validators/email.validator';
import { AuthStateService } from '@/app/store/AuthStateService';
import { AuthService } from '@/app/api/services';
import { ToastService } from '@/app/shared/services/toast.service';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { LoginRequest } from '@/app/api/models';

@Component({
  selector: 'app-login',
  imports: [CommonModule, RouterLink, ReactiveFormsModule, LucideAngularModule],
  templateUrl: './login.html',
})
export class Login {
  private fb = inject(FormBuilder);
  private authState = inject(AuthStateService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private toastService = inject(ToastService);

  readonly icons = LucideIcons;

  showPassword = signal(false);
  submitted = signal(false);
  showResendVerification = signal(false);

  loading = this.authState.loading;

  userForm = this.fb.group({
    email: ['', [Validators.required, Validators.email, strictEmailValidator]],
    password: ['', Validators.required],
    rememberMe: [false],
  });

  onSubmit(event: Event) {
    event.preventDefault();
    this.submitted.set(true);
    if (this.userForm.invalid) {
      return;
    }

    const credentials = this.userForm.value as LoginRequest;
    this.authState.login(credentials).subscribe({
      next: () => {
        if (this.authState.isAuthenticated()) {
          this.toastService.success('Login successful!');
          this.router.navigate(['/dashboard']);
        }
      },
      error: (error) => {
        const errorMessage = error.error?.message || 'Login failed';
        
        if (errorMessage.includes('verify your email')) {
          this.showResendVerification.set(true);
        }
      }
    });
  }

  resendVerification() {
    const email = this.userForm.get('email')?.value;
    if (!email) {
      this.toastService.error('Please enter your email address');
      return;
    }

    this.authService.resendVerification({ email }).subscribe({
      next: () => {
        this.toastService.success('Verification email sent! Please check your inbox.');
        this.showResendVerification.set(false);
      }
    });
  }

  checkFieldError(name: string) {
    const control = this.userForm.get(name);
    return (this.submitted() || control?.touched) && control?.errors;
  }


  togglePassword() {
    this.showPassword.update((v) => !v);
  }
}
