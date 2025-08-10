import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MainLayout } from './shared/components/layouts/main-layout/main-layout';
import { ToastComponent } from './shared/components/ui/toast/toast.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, MainLayout, ToastComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',
})
export class App {}
