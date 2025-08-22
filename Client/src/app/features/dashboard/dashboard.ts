import { Component } from '@angular/core';
import { DashboardLayout } from '@/app/shared/components/layouts/dashboard-layout/dashboard-layout';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css'],
  imports: [DashboardLayout],
})
export class DashboardComponent {}
