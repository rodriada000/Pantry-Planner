import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class MathUtilService {

  constructor() { }

  decimalToFraction(decimal: number): string {
    if (decimal === 0) return "0"; // Special case for 0

    const tolerance = 1.0e-10; // Tolerance for floating-point comparison

    let numerator: number;
    let denominator: number;

    // Handle non-repeating decimals
    for (let i = 1; ; i++) {
      numerator = decimal * i;
      denominator = i;

      const remainder = numerator % 1;
      if (Math.abs(remainder) < tolerance) {
        break;
      }
    }

    // Handle repeating decimals
    if (Math.abs(decimal) < 1) {
      const decimalStr = decimal.toString().split('.')[1];
      const repeatingDigits = decimalStr.match(/(\d+?)\1+$/);

      if (repeatingDigits) {
        const repeatingLength = repeatingDigits[1].length;
        denominator = parseInt('9'.repeat(repeatingLength));
        numerator = Math.round(decimal * denominator);
      }
    }

    const gcd = this.greatestCommonDivisor(numerator, denominator);
    numerator /= gcd;
    denominator /= gcd;

    return `${numerator}/${denominator}`;
  }

  greatestCommonDivisor(a: number, b: number): number {
    if (b === 0) return a;
    return this.greatestCommonDivisor(b, a % b);
  }

}
