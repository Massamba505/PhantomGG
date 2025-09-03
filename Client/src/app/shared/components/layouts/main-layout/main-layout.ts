import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ToastComponent } from '@/app/shared/components/ui/toast/toast';

@Component({
  selector: 'app-main-layout',
  templateUrl: './main-layout.html',
  imports: [CommonModule, RouterModule, ToastComponent],
  standalone: true,
})
export class MainLayout {}
