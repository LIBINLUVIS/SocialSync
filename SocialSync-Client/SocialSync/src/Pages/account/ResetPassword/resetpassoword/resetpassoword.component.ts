import { Component } from '@angular/core';
import { UntypedFormGroup, UntypedFormControl,Validators, FormBuilder,FormGroup,FormControl } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-resetpassoword',
  standalone: true,
  imports: [ReactiveFormsModule,CommonModule],
  templateUrl: './resetpassoword.component.html',
  styleUrl: './resetpassoword.component.scss'
})
export class ResetpassowordComponent {
 
  resetpasswordForm:UntypedFormGroup;

  constructor(private formBuilder:FormBuilder){
    this.resetpasswordForm = this.formBuilder.group({
      newpassword: ['', [Validators.required, Validators.minLength(8)]],
    })

  }

  resetPassword(){
   if(this.resetpasswordForm.valid){
    window.location.href = '/login';
   }
  }
}
