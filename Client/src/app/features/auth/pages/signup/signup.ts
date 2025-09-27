import { Component, computed, inject, signal } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthStateService } from '@/app/store/AuthStateService';
import { ToastService } from '@/app/shared/services/toast.service';
import { strictEmailValidator } from '@/app/shared/validators/email.validator';
import { passwordStrengthValidator } from '@/app/shared/validators/password.validator';
import { matchPasswordsValidator } from '@/app/shared/validators/match-passwords.validator';
import { getPasswordScore } from '@/app/shared/utils/PasswordScore';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';
import { RegisterRequest } from '@/app/api/models';
import { Roles } from '@/app/shared/constants/roles';

@Component({
  selector: 'app-signup',
  imports: [CommonModule, RouterLink, ReactiveFormsModule, LucideAngularModule],
  templateUrl: './signup.html',
})
export class Signup {
  private fb = inject(FormBuilder);
  private authState = inject(AuthStateService);
  private router = inject(Router);
  private toast = inject(ToastService);

  readonly icons = LucideIcons;

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
      role: [Roles.User],
      password: ['', [Validators.required, passwordStrengthValidator]],
      confirmPassword: ['', Validators.required],
    },
    { validators: matchPasswordsValidator }
  );

  get passwordStrength() {
    const value = this.signupForm.controls['password'].value || '';
    const score = getPasswordScore(value);
    console.log(score);
    if (!value) return { label: '', color: '' };
    if (score >= 4)
      return { label: 'Strong Password', color: 'text-[hsl(var(--success))]' };
    if (score >= 3) {
      return { label: 'Medium Strength', color: 'text-[hsl(var(--warning))]' };
    }
    return {
      label: 'Weak Password',
      color: 'text-[hsl(var(--destructive))]',
    };
  }

  onSubmit() {
    this.submitted.set(true);
    if (this.signupForm.invalid) {
      return;
    }

    this.router.navigate(['/auth/role-selection'], {
      state: { signupData: this.signupForm.value }
    });
  }

  togglePassword() {
    this.showPassword.update((x) => !x);
  }

  togglePassword2() {
    this.showPassword2.update((x) => !x);
  }
}
