
import { BaseTable } from './BaseTable.js';
import { BaseClassDataRow } from './BaseClassDataRow.js';

export class BaseClassDataID extends BaseTable {
  static Table = new Map();

  release() {
    this.constructor.Table.clear();
  }

  static loadFromJson(jsonData) {
    this.Table.clear();
    for (const [enumName, rowData] of Object.entries(jsonData)) {
      const enumVal = this._getEnum(enumName);
      const row = this._getRowClass().fromJson(rowData);
      this.Table.set(enumVal, row);
    }
  }

  static _getEnum(name) {
    throw new Error('_getEnum must be implemented in subclass');
  }

  static _getRowClass() {
    throw new Error('_getRowClass must be implemented in subclass');
  }
}
    