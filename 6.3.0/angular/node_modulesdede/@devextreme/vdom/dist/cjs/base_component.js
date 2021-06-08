"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var inferno_1 = require("inferno");
var effect_host_1 = require("./effect_host");
var areObjectsEqual = function (firstObject, secondObject) {
    var bothAreObjects = firstObject instanceof Object && secondObject instanceof Object;
    if (!bothAreObjects) {
        return firstObject === secondObject;
    }
    var firstObjectKeys = Object.keys(firstObject);
    var secondObjectKeys = Object.keys(secondObject);
    if (firstObjectKeys.length !== secondObjectKeys.length) {
        return false;
    }
    var hasDifferentElement = firstObjectKeys.some(function (key) { return firstObject[key] !== secondObject[key]; });
    return !hasDifferentElement;
};
var BaseInfernoComponent = /** @class */ (function (_super) {
    __extends(BaseInfernoComponent, _super);
    function BaseInfernoComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this._pendingContext = _this.context;
        return _this;
    }
    BaseInfernoComponent.prototype.componentWillReceiveProps = function (_, context) {
        this._pendingContext = context !== null && context !== void 0 ? context : {};
    };
    BaseInfernoComponent.prototype.shouldComponentUpdate = function (nextProps, nextState) {
        return (!areObjectsEqual(this.props, nextProps)
            || !areObjectsEqual(this.state, nextState)
            || !areObjectsEqual(this.context, this._pendingContext));
    };
    return BaseInfernoComponent;
}(inferno_1.Component));
exports.BaseInfernoComponent = BaseInfernoComponent;
var InfernoComponent = /** @class */ (function (_super) {
    __extends(InfernoComponent, _super);
    function InfernoComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this._effects = [];
        return _this;
    }
    InfernoComponent.prototype.createEffects = function () {
        return [];
    };
    InfernoComponent.prototype.updateEffects = function () { };
    InfernoComponent.prototype.componentWillMount = function () {
        effect_host_1.InfernoEffectHost.lock();
    };
    InfernoComponent.prototype.componentWillUpdate = function () {
        effect_host_1.InfernoEffectHost.lock();
    };
    InfernoComponent.prototype.componentDidMount = function () {
        var _this = this;
        effect_host_1.InfernoEffectHost.callbacks.push(function () { _this._effects = _this.createEffects(); });
        effect_host_1.InfernoEffectHost.callEffects();
    };
    InfernoComponent.prototype.componentDidUpdate = function () {
        var _this = this;
        effect_host_1.InfernoEffectHost.callbacks.push(function () { return _this.updateEffects(); });
        effect_host_1.InfernoEffectHost.callEffects();
    };
    InfernoComponent.prototype.destroyEffects = function () {
        this._effects.forEach(function (e) { return e.dispose(); });
    };
    InfernoComponent.prototype.componentWillUnmount = function () {
        this.destroyEffects();
    };
    return InfernoComponent;
}(BaseInfernoComponent));
exports.InfernoComponent = InfernoComponent;
var InfernoWrapperComponent = /** @class */ (function (_super) {
    __extends(InfernoWrapperComponent, _super);
    function InfernoWrapperComponent() {
        var _this = _super !== null && _super.apply(this, arguments) || this;
        _this.vDomElement = null;
        _this.vDomPreviousClasses = [];
        _this.vDomRemovedClasses = [];
        _this.vDomAddedClasses = [];
        return _this;
    }
    InfernoWrapperComponent.prototype.vDomUpdateClasses = function () {
        var _this = this;
        var _a;
        var currentClasses = ((_a = this.vDomElement) === null || _a === void 0 ? void 0 : _a.className.length) ? this.vDomElement.className.split(' ')
            : [];
        var addedClasses = currentClasses.filter(function (className) { return _this.vDomPreviousClasses.indexOf(className) < 0; });
        var removedClasses = this.vDomPreviousClasses.filter(function (className) { return currentClasses.indexOf(className) < 0; });
        addedClasses.forEach(function (value) {
            var indexInRemoved = _this.vDomRemovedClasses.indexOf(value);
            if (indexInRemoved > -1) {
                _this.vDomRemovedClasses.splice(indexInRemoved, 1);
            }
            else {
                _this.vDomAddedClasses.push(value);
            }
        });
        removedClasses.forEach(function (value) {
            var indexInAdded = _this.vDomAddedClasses.indexOf(value);
            if (indexInAdded > -1) {
                _this.vDomAddedClasses.splice(indexInAdded, 1);
            }
            else {
                _this.vDomRemovedClasses.push(value);
            }
        });
    };
    InfernoWrapperComponent.prototype.componentDidMount = function () {
        var _a;
        _super.prototype.componentDidMount.call(this);
        this.vDomElement = inferno_1.findDOMfromVNode(this.$LI, true);
        this.vDomPreviousClasses = ((_a = this.vDomElement) === null || _a === void 0 ? void 0 : _a.className.length) ? this.vDomElement.className.split(' ')
            : [];
    };
    InfernoWrapperComponent.prototype.componentDidUpdate = function () {
        var _a;
        _super.prototype.componentDidUpdate.call(this);
        var element = this.vDomElement;
        if (element !== null) {
            this.vDomAddedClasses.forEach(function (className) { return element.classList.add(className); });
            this.vDomRemovedClasses.forEach(function (className) { return element.classList.remove(className); });
            this.vDomPreviousClasses = ((_a = this.vDomElement) === null || _a === void 0 ? void 0 : _a.className.length) ? this.vDomElement.className.split(' ')
                : [];
        }
    };
    InfernoWrapperComponent.prototype.shouldComponentUpdate = function (nextProps, nextState) {
        var shouldUpdate = _super.prototype.shouldComponentUpdate.call(this, nextProps, nextState);
        if (shouldUpdate) {
            this.vDomUpdateClasses();
        }
        return shouldUpdate;
    };
    return InfernoWrapperComponent;
}(InfernoComponent));
exports.InfernoWrapperComponent = InfernoWrapperComponent;
