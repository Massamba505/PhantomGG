import { Component, inject, signal } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/authService';
import { LoginRequest } from '@/app/shared/models/Authentication';
import { strongPasswordValidator } from '../../../../shared/validators/password.validator';

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink, ReactiveFormsModule],
  templateUrl: './login.html',
})
export class Login {
  authService = inject(AuthService);
  showPassword = signal<boolean>(false);
  isLoading = signal<Boolean>(false);
  userForm: FormGroup = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [
      Validators.required,
      strongPasswordValidator(),
    ]),
  });

  onSubmit(event: Event) {
    event.preventDefault();
    if (this.userForm.invalid) {
      alert('invalid');
    }
    this.isLoading.update((x) => !x);
    const userCredentials: LoginRequest = this.userForm.value;
    this.authService.login(userCredentials).subscribe({
      next: (response) => {
        console.log(response);
      },
      error: (error) => {
        console.log(error.message);
        this.isLoading.update((x) => !x);
      },
    });
  }

  togglePassword() {
    this.showPassword.update((x) => !x);
  }
}
