import { Injectable } from "@angular/core";



@Injectable({
  providedIn: 'root'
})

export default class ApistorageService{

  constructor(){

  }
   
  ngOnInit(){
    
  }

  get(key:string):string | null{
    var data = localStorage.getItem(key);
    if(data!=null){
      return data
    }else{
      return null;
    }
  }

 set(key:string,value:string):boolean{
  localStorage.setItem(key,value);
  return true

 }


}



