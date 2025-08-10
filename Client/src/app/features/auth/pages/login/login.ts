import { Component, inject, signal } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { LoginRequest } from '@/app/shared/models/Authentication';
import { strictEmailValidator } from '@/app/shared/validators/email.validator';
import { AuthStateService } from '@/app/store/AuthStateService';

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink, ReactiveFormsModule],
  templateUrl: './login.html',
})
export class Login {
  private authState = inject(AuthStateService);
  private router = inject(Router);

  showPassword = signal(false);
  userForm = new FormGroup({
    email: new FormControl('', [
      Validators.required,
      Validators.email,
      strictEmailValidator,
    ]),
    password: new FormControl('', [Validators.required]),
  });

  loading = this.authState.loading;
  error = this.authState.error;

  onSubmit(event: Event) {
    event.preventDefault();
    if (this.userForm.invalid) {
      return;
    }
    const credentials = this.userForm.value as LoginRequest;
    this.authState.login(credentials).subscribe({
      next: () => {
        if (this.authState.isAuthenticated()) {
          this.router.navigate(['/dashboard']);
        }
      },
    });
  }

  togglePassword() {
    this.showPassword.update((v) => !v);
  }
}
