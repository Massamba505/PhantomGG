import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { LucideAngularModule } from 'lucide-angular';
import { LucideIcons } from '@/app/shared/components/ui/icons/lucide-icons';

@Component({
  selector: 'app-user-dashboard',
  imports: [
    CommonModule,
    RouterModule,
    LucideAngularModule
  ],
  templateUrl: "./user-dashboard.html"
})
export class UserDashboard {
  readonly icons = LucideIcons;
}
