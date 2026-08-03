

export class BaseCustomClassData {
    read(view, offset) {
        throw new Error("read() must be implemented");
    }
    loadJson(data) {
        throw new Error("loadJson() must be implemented");
    }
}
    