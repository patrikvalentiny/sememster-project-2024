import {Component, inject, Input} from '@angular/core';
import {WebsocketService} from "../../services/websocket.service";
import {ClientControlsMotor} from "../../services/events/client/client-controls-motor";
import {StateService} from "../../services/state.service";

@Component({
  selector: 'app-motor-control',
  standalone: true,
  imports: [],
  templateUrl: './motor-control.component.html',
  styleUrl: './motor-control.component.css'
})
export class MotorControlComponent {
  ws = inject(WebsocketService);
  state = inject(StateService);
  @Input() mac: string = "083af23e5a64";


  move(val: any) {
    const value : number= val.value;
    this.ws.sendJson(new ClientControlsMotor({position: value, mac:this.mac}));
  }
}
