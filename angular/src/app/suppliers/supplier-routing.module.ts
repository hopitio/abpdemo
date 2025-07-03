import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SupplierComponent } from './supplier.component';
import { authGuard, permissionGuard } from '@abp/ng.core';

const routes: Routes = [{ 
  path: '', 
  component: SupplierComponent,
  canActivate: [authGuard, permissionGuard],
  data: {
    requiredPolicy: 'AbpAngular.Suppliers'
  }
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SupplierRoutingModule {}
