import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'timefilter',
})
export class TimefilterPipe implements PipeTransform {
  /**
   * Takes a value and makes it lowercase.
   */
  transform(value: string, ...args) {
    return value.toLowerCase();
  }
}
