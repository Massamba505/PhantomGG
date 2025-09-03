import { Component, input, output } from '@angular/core';
import { Modal } from "../modal/modal";
import { LucideAngularModule } from "lucide-angular";
import { LucideIcons } from '../icons/lucide-icons';

@Component({
  selector: 'app-confirm-delete-modal',
  imports: [Modal, LucideAngularModule],
  templateUrl: "./ConfirmDeleteModal.html",
  styleUrl: './ConfirmDeleteModal.css',
})
export class ConfirmDeleteModal { 
  isOpen = input<boolean>(false);
  title = input<string>('Delete');
  entityName = input<string>('');
  close = output<void>();
  confirm = output<void>();

  readonly icons = LucideIcons;

  handleClose() {
    this.close.emit();
  }

  handleConfirm() {
    this.confirm.emit();
  }
}
