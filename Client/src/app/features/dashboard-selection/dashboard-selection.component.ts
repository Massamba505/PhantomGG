import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthStateService } from '@/app/store/AuthStateService';

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
        case 'Organizer':
            this.navigateTo("/organizer");
            break;
        case 'Admin':
            this.navigateTo("/admin");
            break;
        case 'User':
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
