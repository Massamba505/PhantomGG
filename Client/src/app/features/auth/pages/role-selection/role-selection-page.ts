import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthStateService } from '@/app/store/AuthStateService';
import { ToastService } from '@/app/shared/services/toast.service';
import { RoleSelection } from '@/app/shared/components/role-selection/role-selection';
import { RegisterRequest, UserRoles } from '@/app/api/models';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-role-selection-page',
  templateUrl: './role-selection-page.html',
  imports: [CommonModule, RoleSelection, LucideAngularModule],
})
export class RoleSelectionPage {
  private router = inject(Router);
  private authState = inject(AuthStateService);
  private toast = inject(ToastService);

  readonly icons = LucideIcons;
  readonly UserRoles = UserRoles;
  selectedRole = signal<UserRoles | null>(null);
  signupData: any = null;
  loading = this.authState.loading;

  constructor() {
    const navigation = this.router.getCurrentNavigation();
    if (navigation?.extras.state?.['signupData']) {
      this.signupData = navigation.extras.state['signupData'];
    } else {
      this.router.navigate(['/auth/signup']);
    }
  }

  onRoleSelected(role: UserRoles) {
    this.selectedRole.set(role);
  }

  onContinue() {
    if (!this.selectedRole() || !this.signupData) {
      this.toast.error('Please select an account type');
      return;
    }

    const credentials: RegisterRequest = {
      ...this.signupData,
      role: this.selectedRole()!
    };

    this.authState.register(credentials).subscribe({
      next: () => {
        this.router.navigate(['/auth/verify-email-sent'], {
          state: { email: credentials.email }
        });
      },
      error: (error) => {
        this.router.navigate(['/auth/signup'],{
          state : { signupData: this.signupData }
        });
      }
    });
  }

  onBack() {
    this.router.navigate(['/auth/signup']);
  }
}