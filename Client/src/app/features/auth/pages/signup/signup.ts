import { Component, computed, inject, signal } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthStateService } from '@/app/store/AuthStateService';
import { ToastService } from '@/app/shared/services/toast.service';
import { primengModules } from '@/app/shared/components/primeng/primeng-config';
import { strictEmailValidator } from '@/app/shared/validators/email.validator';
import { passwordStrengthValidator } from '@/app/shared/validators/password.validator';
import { matchPasswordsValidator } from '@/app/shared/validators/match-passwords.validator';
import { NgClass } from '@angular/common';
import { getPasswordScore } from '@/app/shared/utils/PasswordScore';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [RouterLink, NgClass, ReactiveFormsModule, ...primengModules],
  templateUrl: './signup.html',
})
export class Signup {
  private fb = inject(FormBuilder);
  private authState = inject(AuthStateService);
  private router = inject(Router);
  private toast = inject(ToastService);

  showPassword = signal(false);
  showPassword2 = signal(false);
  submitted = signal(false);

  loading = this.authState.loading;

  signupForm: FormGroup = this.fb.group(
    {
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: [
        '',
        [Validators.required, Validators.email, strictEmailValidator],
      ],
      password: ['', [Validators.required, passwordStrengthValidator]],
      confirmPassword: ['', Validators.required],
    },
    { validators: matchPasswordsValidator }
  );

  get passwordStrength() {
    const value = this.signupForm.controls['password'].value || '';
    const score = getPasswordScore(value);
    if (!value) return { label: '', color: '' };
    if (score >= 4)
      return { label: 'Strong Password 💪', color: 'text-green-600' };
    if (score >= 3)
      return { label: 'Medium Strength ⚠️', color: 'text-yellow-600' };
    return { label: 'Weak Password 😢', color: 'text-red-600' };
  }

  onSubmit() {
    this.submitted.set(true);
    if (this.signupForm.invalid) {
      return;
    }

    const { firstName, lastName, email, password } = this.signupForm.value;
    this.authState.signup({ firstName, lastName, email, password }).subscribe({
      next: () => {
        if (this.authState.isAuthenticated()) {
          this.router.navigate(['/dashboard']);
        }
      },
      error: (err) => {
        this.toast.error(err.message || 'Signup failed.');
      },
    });
  }

  togglePassword() {
    this.showPassword.update((x) => !x);
  }

  togglePassword2() {
    this.showPassword2.update((x) => !x);
  }
}
