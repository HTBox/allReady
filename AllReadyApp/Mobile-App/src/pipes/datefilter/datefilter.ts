import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'dateFilterPipe',
})
export class DateFilterPipe implements PipeTransform {
  transform(value, args) {
    let dateValue = new Date(value);    
   
    let transformedDate: string = this.getNameOfWeekday(dateValue) + ", ";
    transformedDate = transformedDate + this.getNameOfMonth(dateValue) + " ";
    transformedDate = transformedDate + dateValue.getUTCDate() + " ";
    transformedDate = transformedDate + dateValue.getUTCFullYear();
    return transformedDate;
  }

  getNameOfMonth(date: Date): string {    
    switch (date.getUTCMonth()) {
      case 0:
        return "January"
      case 1:
        return "February"
      case 2:
        return "March"
      case 3:
        return "April"
      case 4:
        return "May"
      case 5:
        return "June"
      case 6:
        return "July"
      case 7:
        return "August"
      case 8:
        return "September"
      case 9:
        return "October"
      case 10:
        return "November"
      case 11:
        return "December"
      default:
        return ""
    }
  }

  getNameOfWeekday(date: Date) {
    let nameOfWeekday: string;
    switch (date.getUTCDay()) {
      case 0:
        nameOfWeekday = "Sunday"
        break;
      case 1:
        nameOfWeekday = "Monday"
        break;
      case 2:
        nameOfWeekday = "Tuesday"
        break;
      case 3:
        nameOfWeekday = "Wednesday"
        break;
      case 4:
        nameOfWeekday = "Thursday"
        break;
      case 5:
        nameOfWeekday = "Friday"
        break;
      case 6:
        nameOfWeekday = "Saturday"
        break;
      default:
        break;
    };
    return nameOfWeekday;
  }

}
