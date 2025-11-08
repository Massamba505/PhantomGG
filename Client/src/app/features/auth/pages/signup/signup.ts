import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthStateService } from '@/app/store/AuthStateService';
import { strictEmailValidator } from '@/app/shared/validators/email.validator';
import { passwordStrengthValidator } from '@/app/shared/validators/password.validator';
import { matchPasswordsValidator } from '@/app/shared/validators/match-passwords.validator';
import { getPasswordScore } from '@/app/shared/utils/PasswordScore';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { UserRoles } from '@/app/api/models';

@Component({
  selector: 'app-signup',
  imports: [CommonModule, RouterLink, ReactiveFormsModule, LucideAngularModule],
  templateUrl: './signup.html',
})
export class Signup {
  private readonly fb = inject(FormBuilder);
  private readonly authState = inject(AuthStateService);
  private readonly router = inject(Router);

  readonly icons = LucideIcons;

  showPassword = signal(false);
  showConfirmPassword = signal(false);
  submitted = signal(false);

  loading = this.authState.loading;

  constructor() {
    const navigation = this.router.getCurrentNavigation();
    if (navigation?.extras.state?.['signupData']) {
      this.signupForm.setValue({ ...navigation.extras.state['signupData'] });
    }
  }

  signupForm = this.fb.group(
    {
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: [
        '',
        [Validators.required, Validators.email, strictEmailValidator],
      ],
      role: [UserRoles.User],
      password: ['', [Validators.required, passwordStrengthValidator]],
      confirmPassword: ['', Validators.required],
    },
    { validators: matchPasswordsValidator }
  );

  get passwordStrength() {
    const value = this.signupForm.controls['password'].value || '';
    const score = getPasswordScore(value);
    if (!value) return { label: '', color: '' };
    if (score >= 5) return { label: 'Strong Password', color: 'text-success' };
    if (score >= 3) {
      return { label: 'Medium Strength', color: 'text-warning' };
    }
    return {
      label: 'Weak Password',
      color: 'text-destructive',
    };
  }

  onSubmit() {
    this.submitted.set(true);
    if (this.signupForm.invalid) {
      return;
    }

    this.router.navigate(['/auth/role-selection'], {
      state: { signupData: this.signupForm.value },
    });
  }

  togglePassword() {
    this.showPassword.update((x) => !x);
  }

  toggleConfirmPassword() {
    this.showConfirmPassword.update((x) => !x);
  }

  checkFieldError(name: string) {
    const control = this.signupForm.get(name);
    return (this.submitted() || control?.touched) && control?.errors;
  }
}
