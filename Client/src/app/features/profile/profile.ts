import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthStateService } from '@/app/store/AuthStateService';
import { ToastService } from '@/app/shared/services/toast.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="card p-4 max-w-xl mx-auto">
      @if (user()) {
      <div header="My Profile">
        <div class="space-y-4">
          <h3 class="text-lg font-medium">Personal Information</h3>

          <div class="flex justify-between">
            <span class="font-semibold">Name:</span>
            <span>{{ user()!.firstName }} {{ user()!.lastName }}</span>
          </div>

          <div class="flex justify-between">
            <span class="font-semibold">Email:</span>
            <span>{{ user()!.email }}</span>
          </div>

          <div class="flex justify-between">
            <span class="font-semibold">Role:</span>
            <span>{{ user()!.role }}</span>
          </div>

          <div class="text-right">
            <button
              pButton
              label="Edit Profile"
              icon="pi pi-user-edit"
            ></button>
          </div>
        </div>
        <button (click)="logout()" class="btn btn-destructive" type="button">
          Logout
        </button>
      </div>
      }
    </div>
  `,
})
export class Profile {
  private authState = inject(AuthStateService);
  private toast = inject(ToastService);
  private router = inject(Router);

  user = this.authState.user;

  logout() {
    this.authState.logout();
    this.router.navigate(['/']);
    this.toast.success('Logout Successfully.');
  }
}
