import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TestAPIComponent } from './test-api/test-api.component';


const routes: Routes = [
  { path: '', component: TestAPIComponent, pathMatch: 'full' },
  { path: '**', redirectTo: '/' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
