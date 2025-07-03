import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { SupplierRoutingModule } from './supplier-routing.module';
import { SupplierComponent } from './supplier.component';

@NgModule({
  declarations: [SupplierComponent],
  imports: [SharedModule, SupplierRoutingModule],
})
export class SupplierModule {}
