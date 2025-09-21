import { Component, inject, signal } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { strictEmailValidator } from '@/app/shared/validators/email.validator';
import { AuthStateService } from '@/app/store/AuthStateService';
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
  private router = inject(Router);
  private toastService = inject(ToastService);

  readonly icons = LucideIcons;

  showPassword = signal(false);
  submitted = signal(false);

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
    });
  }

  togglePassword() {
    this.showPassword.update((v) => !v);
  }
}
