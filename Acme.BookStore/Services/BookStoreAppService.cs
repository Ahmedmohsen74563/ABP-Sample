using Volo.Abp.Application.Services;
using Acme.BookStore.Localization;

namespace Acme.BookStore.Services;

/* Inherit your application services from this class. */
public abstract class BookStoreAppService : ApplicationService
{
    protected BookStoreAppService()
    {
        LocalizationResource = typeof(BookStoreResource);
    }
}