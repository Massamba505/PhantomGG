import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthStateService } from '@/app/store/AuthStateService';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="card p-4 max-w-xl mx-auto">
      @if (user) {
      <div header="My Profile">
        <div class="space-y-4">
          <h3 class="text-lg font-medium">Personal Information</h3>

          <div class="flex justify-between">
            <span class="font-semibold">Name:</span>
            <span>{{ user.firstName }} {{ user.lastName }}</span>
          </div>

          <div class="flex justify-between">
            <span class="font-semibold">Email:</span>
            <span>{{ user.email }}</span>
          </div>

          <div class="flex justify-between">
            <span class="font-semibold">Role:</span>
            <span>{{ user.role }}</span>
          </div>

          <div class="text-right">
            <button
              pButton
              label="Edit Profile"
              icon="pi pi-user-edit"
            ></button>
          </div>
        </div>
      </div>
      }
    </div>
  `,
})
export class ProfileComponent implements OnInit {
  authState = inject(AuthStateService);
  user = this.authState.user();

  ngOnInit() {
    console.log('Profile component initialized');
  }
}
