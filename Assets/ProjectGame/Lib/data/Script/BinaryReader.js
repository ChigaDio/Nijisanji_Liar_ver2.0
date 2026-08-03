
export class BinaryReader {
    constructor(buffer) {
        this._buffer = buffer;   // ArrayBuffer 推奨
        this._offset = 0;
    }

    readInt32() {
        const value = new Int32Array(this._buffer, this._offset, 1)[0];
        this._offset += 4;
        return value;
    }
    
    readInt16() {
        const value = new Int16Array(this._buffer, this._offset, 1)[0];
        this._offset += 2;
        return value;
    }
    
    readInt64() {
        const value = new BigInt64Array(this._buffer, this._offset, 1)[0];
        this._offset += 8;
        return value;

    }

    readFloat32() {
        const value = new Float32Array(this._buffer, this._offset, 1)[0];
        this._offset += 4;
        return value;
    }

    readBoolean() {
        const value = new Uint8Array(this._buffer, this._offset, 1)[0] !== 0;
        this._offset += 1;
        return value;
    }

    readString() {
        const len = this.readInt32();
        if (len <= 0) return "";
        const bytes = new Uint8Array(this._buffer, this._offset, len);
        this._offset += len;
        return new TextDecoder("utf-8").decode(bytes);
    }

    readDouble() {
        const value = new Float64Array(this._buffer, this._offset, 1)[0];
        this._offset += 8;
        return value;
    }

    readUint() {
        const value = new Uint32Array(this._buffer, this._offset, 1)[0];
        this._offset += 4;
        return value;
    }

    readVector2() {
        return {
            x: this.readFloat32(),
            y: this.readFloat32()
        };
    }

    readVector3() {
        return {
            x: this.readFloat32(),
            y: this.readFloat32(),
            z: this.readFloat32()
        };
    }
    
    readChar()
    {
        const value = new Uint16Array(this._buffer, this._offset, 1)[0];
        this._offset += 2;
        return value;

    }
}
    