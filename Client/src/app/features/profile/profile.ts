import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthStateService } from '@/app/store/AuthStateService';
import { ToastService } from '@/app/shared/services/toast.service';
import { Router, RouterLink } from '@angular/router';
import { DashboardLayout } from '@/app/shared/components/layouts/dashboard-layout/dashboard-layout';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Modal } from '@/app/shared/components/modal/modal';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, DashboardLayout, RouterLink, ReactiveFormsModule, Modal, LucideAngularModule],
  templateUrl: './profile.html',
  styleUrls: ['./profile.css']
})
export class Profile {
  private authState = inject(AuthStateService);
  private toast = inject(ToastService);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  readonly icons = LucideIcons;

  user = this.authState.user;
  isEditProfileModalOpen = signal(false);
  isChangePasswordModalOpen = signal(false);

  stats = {
    activeTournaments: 3,
    teamsManaged: 8,
    completedTournaments: 2
  };

  recentActivity = [
    {
      type: 'tournament',
      message: 'Created tournament "Summer League 2024"',
      date: 'August 20, 2025',
      link: '/tournaments/1'
    },
    {
      type: 'team',
      message: 'Added team "FC Barcelona Academy" to "Summer League 2024"',
      date: 'August 18, 2025',
      link: '/tournaments/1'
    },
    {
      type: 'tournament',
      message: 'Updated tournament "Champions Cup" settings',
      date: 'August 15, 2025',
      link: '/tournaments/2'
    },
    {
      type: 'account',
      message: 'Updated profile information',
      date: 'August 10, 2025',
      link: null
    },
    {
      type: 'tournament',
      message: 'Completed tournament "Spring Invitational"',
      date: 'July 30, 2025',
      link: '/tournaments/101'
    }
  ];

  profileForm: FormGroup = this.fb.group({
    firstName: [this.user()?.firstName || '', Validators.required],
    lastName: [this.user()?.lastName || '', Validators.required],
    email: [this.user()?.email || '', [Validators.required, Validators.email]]
  });

  passwordForm: FormGroup = this.fb.group({
    currentPassword: ['', Validators.required],
    newPassword: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', Validators.required]
  }, {
    validators: this.passwordMatchValidator
  });

  confirmLogout() {
    if (confirm('Are you sure you want to logout?')) {
      this.logout();
    }
  }

  logout() {
    this.authState.logout();
    this.router.navigate(['/']);
    this.toast.success('Logged out successfully');
  }

  updateProfile() {
    if (this.profileForm.invalid) {
      return;
    }
    
    // Simulate updating profile
    setTimeout(() => {
      // Would normally call a service here
      this.isEditProfileModalOpen.set(false);
      this.toast.success('Profile updated successfully');
    }, 500);
  }

  updatePassword() {
    if (this.passwordForm.invalid) {
      return;
    }
    
    // Simulate updating password
    setTimeout(() => {
      // Would normally call a service here
      this.isChangePasswordModalOpen.set(false);
      this.toast.success('Password updated successfully');
      this.passwordForm.reset();
    }, 500);
  }

  passwordMatchValidator(g: FormGroup) {
    const newPassword = g.get('newPassword')?.value;
    const confirmPassword = g.get('confirmPassword')?.value;
    
    return newPassword === confirmPassword ? null : { mismatch: true };
  }
}
