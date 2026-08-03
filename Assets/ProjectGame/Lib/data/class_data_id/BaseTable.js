
export class BaseTable {
  read(reader) { throw new Error('read must be implemented'); }
  release() { throw new Error('release must be implemented'); }
}
    