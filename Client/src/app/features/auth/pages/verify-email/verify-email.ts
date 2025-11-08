import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '@/app/api/services';
import { ToastService } from '@/app/shared/services/toast.service';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-verify-email',
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './verify-email.html',
})
export class VerifyEmail implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly toastService = inject(ToastService);

  readonly icons = LucideIcons;

  verifying = signal(true);
  success = signal(false);
  error = signal('');

  ngOnInit() {
    const token = this.route.snapshot.queryParams['token'];

    if (!token) {
      this.error.set('Verification token is missing');
      this.verifying.set(false);
      return;
    }

    this.verifyEmail(token);
  }

  private verifyEmail(token: string) {
    this.authService.verifyEmail({ token }).subscribe({
      next: () => {
        this.success.set(true);
        this.verifying.set(false);
        this.toastService.success(
          'Email verified successfully! You can now log in.'
        );
        setTimeout(() => {
          this.router.navigate(['/auth/login']);
        }, 3000);
      },
      error: (err) => {
        this.error.set(
          err.error?.message ||
            'Verification failed. The token may be invalid or expired.'
        );
        this.verifying.set(false);
      },
    });
  }

  goToLogin() {
    this.router.navigate(['/auth/login']);
  }
}
