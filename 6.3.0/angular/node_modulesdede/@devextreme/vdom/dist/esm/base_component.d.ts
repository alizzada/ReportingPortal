import { Component } from 'inferno';
import { InfernoEffect } from './effect';
export declare class BaseInfernoComponent<P = Record<string, unknown>, S = Record<string, unknown>> extends Component<P, S> {
    _pendingContext: any;
    componentWillReceiveProps(_: any, context: any): void;
    shouldComponentUpdate(nextProps: P, nextState: S): boolean;
}
export declare class InfernoComponent<P = Record<string, unknown>, S = Record<string, unknown>> extends BaseInfernoComponent<P, S> {
    _effects: InfernoEffect[];
    createEffects(): InfernoEffect[];
    updateEffects(): void;
    componentWillMount(): void;
    componentWillUpdate(): void;
    componentDidMount(): void;
    componentDidUpdate(): void;
    destroyEffects(): void;
    componentWillUnmount(): void;
}
export declare class InfernoWrapperComponent<P = Record<string, unknown>, S = Record<string, unknown>> extends InfernoComponent<P, S> {
    vDomElement: Element | null;
    vDomPreviousClasses: string[];
    vDomRemovedClasses: string[];
    vDomAddedClasses: string[];
    vDomUpdateClasses(): void;
    componentDidMount(): void;
    componentDidUpdate(): void;
    shouldComponentUpdate(nextProps: P, nextState: S): boolean;
}
