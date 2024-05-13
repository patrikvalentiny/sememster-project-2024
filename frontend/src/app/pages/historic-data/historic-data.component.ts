import {Component, Input, OnInit} from '@angular/core';

@Component({
  selector: 'app-historic-data',
  standalone: true,
  imports: [],
  templateUrl: './historic-data.component.html',
  styleUrl: './historic-data.component.css'
})
export class HistoricDataComponent implements OnInit{
  @Input() mac!: string;

  ngOnInit(): void {
    console.log(this.mac)
  }

}
