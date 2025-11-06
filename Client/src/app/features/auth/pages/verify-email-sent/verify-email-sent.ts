import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '@/app/api/services';
import { ToastService } from '@/app/shared/services/toast.service';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-verify-email-sent',
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './verify-email-sent.html',
})
export class VerifyEmailSent {
  private authService = inject(AuthService);
  private router = inject(Router);
  private toastService = inject(ToastService);

  readonly icons = LucideIcons;

  email = signal('');
  resending = signal(false);

  constructor() {
    const navigation = this.router.getCurrentNavigation();
    if (navigation?.extras.state?.['email']) {
      this.email.set(navigation.extras.state['email']);
    } else {
      this.router.navigate(['/auth/signup']);
    }
  }

  resendVerification() {
    if (!this.email()) return;

    this.resending.set(true);
    this.authService.resendVerification({ email: this.email() }).subscribe({
      next: () => {
        this.resending.set(false);
        this.toastService.success('Verification email sent!');
      },
      error: (err) => {
        this.resending.set(false);
        this.toastService.error(
          err.error?.message || 'Failed to resend verification email'
        );
      },
    });
  }

  goToLogin() {
    this.router.navigate(['/auth/login']);
  }
}
