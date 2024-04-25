import {Component, Input} from '@angular/core';
import {Device} from "../../models/device";
import {DatePipe, NgClass} from "@angular/common";
import {BmeData} from "../../models/bme-data";

@Component({
  selector: 'app-device-card',
  standalone: true,
  imports: [
    NgClass,
    DatePipe
  ],
  templateUrl: './device-card.component.html',
  styleUrl: './device-card.component.css'
})
export class DeviceCardComponent {
  @Input() device: Device = {id: 0, name: "distinguished-yak", mac: "69420", status_id: 0, status: "offline"};
  @Input() bmeData?: BmeData = {temperatureC: 0, humidity: 0, pressure: 0, createdAt: new Date()};

}
