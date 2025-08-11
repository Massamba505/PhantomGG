import { Component, inject, signal } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { LoginRequest } from '@/app/shared/models/Authentication';
import { strictEmailValidator } from '@/app/shared/validators/email.validator';
import { AuthStateService } from '@/app/store/AuthStateService';
import { primengModules } from '@/app/shared/components/primeng/primeng-config';
import { ToastService } from '@/app/shared/services/toast.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, ...primengModules],
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
          //this.router.navigate(['/dashboard']);
          console.log('User is authenticated', this.authState.user());
        }
      },
      error: (error) => {
        this.toastService.error(error.error.message);
      },
    });
  }

  togglePassword() {
    this.showPassword.update((v) => !v);
  }
}
