import { ReportingPortalTemplatePage } from './app.po';

describe('ReportingPortal App', function() {
  let page: ReportingPortalTemplatePage;

  beforeEach(() => {
    page = new ReportingPortalTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
