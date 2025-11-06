import { Component } from '@angular/core';
import { UserLayout } from '../../shared/components/layouts/user-layout/user-layout';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css'],
  imports: [UserLayout, RouterOutlet],
})
export class Dashboard {}
