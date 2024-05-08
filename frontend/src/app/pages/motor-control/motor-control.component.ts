import {Component, inject, Input} from '@angular/core';
import {WebsocketService} from "../../services/websocket.service";
import {ClientControlsMotor} from "../../services/events/client/client-controls-motor";
import {StateService} from "../../services/state.service";
import {NgClass} from "@angular/common";

@Component({
  selector: 'app-motor-control',
  standalone: true,
  imports: [
    NgClass
  ],
  templateUrl: './motor-control.component.html',
  styleUrl: './motor-control.component.css'
})
export class MotorControlComponent {
  ws = inject(WebsocketService);
  state = inject(StateService);
  @Input() mac?: string
  value: number = 0;


  async move(val: any) {
    this.value = val.value;
    this.state.motorMoving.set(this.mac!, true);
    this.ws.sendJson(new ClientControlsMotor({position: this.value, mac:this.mac}));
  }
}
