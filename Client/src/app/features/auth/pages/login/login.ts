import { Component, inject, signal } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LoginRequest } from '@/app/shared/models/Authentication';
import { strictEmailValidator } from '@/app/shared/validators/email.validator';
import { AuthStateService } from '@/app/store/AuthStateService';
import { ToastService } from '@/app/shared/services/toast.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule],
  templateUrl: './login.html',
})
export class Login {
  private fb = inject(FormBuilder);
  private authState = inject(AuthStateService);
  private router = inject(Router);
  private toastService = inject(ToastService);

  showPassword = signal(false);
  submitted = signal(false);

  loading = this.authState.loading;
  error = this.authState.error;

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
        this.toastService.error(error.error?.message || 'Login failed. Please check your credentials.');
      },
    });
  }

  togglePassword() {
    this.showPassword.update((v) => !v);
  }
}
