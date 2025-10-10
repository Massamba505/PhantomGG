import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthStateService } from '@/app/store/AuthStateService';
import { UserRoles } from '@/app/api/models';

@Component({
  selector: 'app-dashboard-selection',
  imports: [CommonModule],
  template: '',
})
export class DashboardSelectionComponent implements OnInit {
  private router = inject(Router);
  private authState = inject(AuthStateService);
  user = this.authState.user;

  ngOnInit() {
      switch(this.user()?.role){
        case UserRoles.Organizer:
            this.navigateTo("/organizer");
            break;
        case UserRoles.Admin:
            this.navigateTo("/admin");
            break;
        case UserRoles.User:
            this.navigateTo("/user");
            break;
        default:
            this.navigateTo("/unathorized");
      }
  }

  navigateTo(route: string) {
    this.router.navigate([route]);
  }
}
