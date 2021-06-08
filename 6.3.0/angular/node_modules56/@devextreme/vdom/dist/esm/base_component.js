import { Component, findDOMfromVNode } from 'inferno';
import { InfernoEffectHost } from './effect_host';
const areObjectsEqual = (firstObject, secondObject) => {
    const bothAreObjects = firstObject instanceof Object && secondObject instanceof Object;
    if (!bothAreObjects) {
        return firstObject === secondObject;
    }
    const firstObjectKeys = Object.keys(firstObject);
    const secondObjectKeys = Object.keys(secondObject);
    if (firstObjectKeys.length !== secondObjectKeys.length) {
        return false;
    }
    const hasDifferentElement = firstObjectKeys.some((key) => firstObject[key] !== secondObject[key]);
    return !hasDifferentElement;
};
export class BaseInfernoComponent extends Component {
    constructor() {
        super(...arguments);
        this._pendingContext = this.context;
    }
    componentWillReceiveProps(_, context) {
        this._pendingContext = context !== null && context !== void 0 ? context : {};
    }
    shouldComponentUpdate(nextProps, nextState) {
        return (!areObjectsEqual(this.props, nextProps)
            || !areObjectsEqual(this.state, nextState)
            || !areObjectsEqual(this.context, this._pendingContext));
    }
}
export class InfernoComponent extends BaseInfernoComponent {
    constructor() {
        super(...arguments);
        this._effects = [];
    }
    createEffects() {
        return [];
    }
    updateEffects() { }
    componentWillMount() {
        InfernoEffectHost.lock();
    }
    componentWillUpdate() {
        InfernoEffectHost.lock();
    }
    componentDidMount() {
        InfernoEffectHost.callbacks.push(() => { this._effects = this.createEffects(); });
        InfernoEffectHost.callEffects();
    }
    componentDidUpdate() {
        InfernoEffectHost.callbacks.push(() => this.updateEffects());
        InfernoEffectHost.callEffects();
    }
    destroyEffects() {
        this._effects.forEach((e) => e.dispose());
    }
    componentWillUnmount() {
        this.destroyEffects();
    }
}
export class InfernoWrapperComponent extends InfernoComponent {
    constructor() {
        super(...arguments);
        this.vDomElement = null;
        this.vDomPreviousClasses = [];
        this.vDomRemovedClasses = [];
        this.vDomAddedClasses = [];
    }
    vDomUpdateClasses() {
        var _a;
        const currentClasses = ((_a = this.vDomElement) === null || _a === void 0 ? void 0 : _a.className.length) ? this.vDomElement.className.split(' ')
            : [];
        const addedClasses = currentClasses.filter((className) => this.vDomPreviousClasses.indexOf(className) < 0);
        const removedClasses = this.vDomPreviousClasses.filter((className) => currentClasses.indexOf(className) < 0);
        addedClasses.forEach((value) => {
            const indexInRemoved = this.vDomRemovedClasses.indexOf(value);
            if (indexInRemoved > -1) {
                this.vDomRemovedClasses.splice(indexInRemoved, 1);
            }
            else {
                this.vDomAddedClasses.push(value);
            }
        });
        removedClasses.forEach((value) => {
            const indexInAdded = this.vDomAddedClasses.indexOf(value);
            if (indexInAdded > -1) {
                this.vDomAddedClasses.splice(indexInAdded, 1);
            }
            else {
                this.vDomRemovedClasses.push(value);
            }
        });
    }
    componentDidMount() {
        var _a;
        super.componentDidMount();
        this.vDomElement = findDOMfromVNode(this.$LI, true);
        this.vDomPreviousClasses = ((_a = this.vDomElement) === null || _a === void 0 ? void 0 : _a.className.length) ? this.vDomElement.className.split(' ')
            : [];
    }
    componentDidUpdate() {
        var _a;
        super.componentDidUpdate();
        const element = this.vDomElement;
        if (element !== null) {
            this.vDomAddedClasses.forEach((className) => element.classList.add(className));
            this.vDomRemovedClasses.forEach((className) => element.classList.remove(className));
            this.vDomPreviousClasses = ((_a = this.vDomElement) === null || _a === void 0 ? void 0 : _a.className.length) ? this.vDomElement.className.split(' ')
                : [];
        }
    }
    shouldComponentUpdate(nextProps, nextState) {
        const shouldUpdate = super.shouldComponentUpdate(nextProps, nextState);
        if (shouldUpdate) {
            this.vDomUpdateClasses();
        }
        return shouldUpdate;
    }
}
