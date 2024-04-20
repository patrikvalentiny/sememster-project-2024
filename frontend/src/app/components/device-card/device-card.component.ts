import {Component, Input} from '@angular/core';
import {Device} from "../../models/device";
import {NgClass} from "@angular/common";

@Component({
  selector: 'app-device-card',
  standalone: true,
  imports: [
    NgClass
  ],
  templateUrl: './device-card.component.html',
  styleUrl: './device-card.component.css'
})
export class DeviceCardComponent {
  @Input() device: Device = {id: 0, name: "distinguished-yak", mac: "69420", status_id: 0, status: "offline"};


}
