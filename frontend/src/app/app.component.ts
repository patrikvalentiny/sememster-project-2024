import {Component} from '@angular/core';
import {HomeSkeletonComponent} from "./pages/home/home-skeleton/home-skeleton.component";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  imports: [
    HomeSkeletonComponent
  ],
  standalone: true
})
export class AppComponent {
}
