import { Injectable, TemplateRef } from '@angular/core';
import { MessageService } from 'primeng/api';

@Injectable({
  providedIn: 'root'
})
export class ToastService {

  constructor(private toasts: MessageService) {

  }

  show(message: string, severity: string = "info", summary: string = "") {
    this.toasts.add({severity: severity, summary: summary, detail: message})
  }

  showStandard(msg: string, title: string = "") {
    this.toasts.add({severity: "info", summary: title, detail: msg, life: 5000});
  }

  showSuccess(msg: string, title: string = "") {
    this.toasts.add({severity: "success", summary: title, detail: msg, life: 5000});
  }

  showDanger(msg: string, title: string = "") {
    this.toasts.add({severity: "error", summary: title, detail: msg, life: 8000});
  }
  
}
