import {Component, inject, Input} from '@angular/core';
import {Device} from "../../models/device";
import {Router} from "@angular/router";

@Component({
  selector: 'app-device-sidebar-item',
  standalone: true,
  imports: [],
  templateUrl: './device-sidebar-item.component.html',
  styleUrl: './device-sidebar-item.component.css'
})
export class DeviceSidebarItemComponent {
  private readonly router = inject(Router);
  @Input() device!: Device ;

  viewData() {

  }

  async controlMotor() {
    await this.router.navigate(["/motor", this.device.mac]);
  }
}
