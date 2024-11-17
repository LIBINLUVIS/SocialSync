import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { UntypedFormGroup, UntypedFormControl,Validators, FormBuilder,FormGroup,FormControl } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';


@Component({
  selector: 'app-forgotpassword-page',
  standalone: true,
  imports: [ReactiveFormsModule,CommonModule],
  templateUrl: './forgotpassword-page.component.html',
  styleUrl: './forgotpassword-page.component.scss'
})
export class ForgotpasswordPageComponent implements OnInit {

  forgotpasswordForm:UntypedFormGroup;
  verifycodeForm:UntypedFormGroup;
  isverificationCode:boolean = false;
  isSubmitting: boolean = false;

  constructor(private formBuilder:FormBuilder){
    this.forgotpasswordForm = this.formBuilder.group({
      useremail: ['', [Validators.required, Validators.email]],
    })
    this.verifycodeForm = this.formBuilder.group({
      Code: ['', [Validators.required]],
    })
  }
  ngOnInit():void{
     
  }

  sendemailVerification(){
    if(this.forgotpasswordForm.valid){
      this.isverificationCode = true;
      this.isSubmitting = true;
      this.forgotpasswordForm.get('useremail')?.disable();
      console.log(this.forgotpasswordForm.value['useremail'])
    }
    

  }

  verifyingCode(){
     if(this.verifycodeForm.valid){
      window.location.href = '/resetpassword';
     }
  }





}
