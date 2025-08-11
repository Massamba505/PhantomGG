import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { AuthStateService } from '@/app/store/AuthStateService';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, CardModule, ButtonModule],
  template: `
    <div class="flex justify-center p-4">
      <p-card
        *ngIf="authState.user()"
        header="My Profile"
        styleClass="w-full max-w-xl"
      >
        <div class="p-4">
          <div class="mb-4">
            <h3 class="text-lg font-medium">Personal Information</h3>
            <div class="flex flex-col gap-2 mt-2">
              <div class="flex justify-between">
                <span class="font-semibold">Name:</span>
                <span
                  >{{ authState.user()?.firstName }}
                  {{ authState.user()?.lastName }}</span
                >
              </div>
              <div class="flex justify-between">
                <span class="font-semibold">Email:</span>
                <span>{{ authState.user()?.email }}</span>
              </div>
              <div class="flex justify-between">
                <span class="font-semibold">Role:</span>
                <span>{{ authState.user()?.role }}</span>
              </div>
            </div>
          </div>

          <div class="flex justify-end mt-4">
            <button
              pButton
              label="Edit Profile"
              icon="pi pi-user-edit"
            ></button>
          </div>
        </div>
      </p-card>
    </div>
  `,
})
export class ProfileComponent implements OnInit {
  authState = inject(AuthStateService);

  ngOnInit() {
    console.log('Profile component initialized');
  }
}
