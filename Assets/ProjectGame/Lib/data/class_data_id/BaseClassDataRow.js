
export class BaseClassDataRow {
  read(reader) { throw new Error('read must be implemented'); }
  static fromJson(data) { throw new Error('fromJson must be implemented'); }
}
    