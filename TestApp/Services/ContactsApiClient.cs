using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using TestApp.Models;

namespace TestApp.Services
{
  public class ContactsApiClient
  {
    private HttpClient client;

    public ContactsApiClient(HttpClient client)
    {
      this.client = client;
      //client = new HttpClient();
      //client.BaseAddress = new Uri("http://localhost:54669/");
      //client.DefaultRequestHeaders.Accept.Add(
      //    new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public IObservable<ImmutableList<Contact>> GetAllContacts()
    {
      return Observable.FromAsync(async token =>
      {
        var response = await client.GetAsync("api/contacts", token);
        if (response.IsSuccessStatusCode)
        {
          return await response.Content.ReadAsAsync<ImmutableList<Contact>>();
        }
        else
        {
          throw new Exception("something went wrong when getting the contacts");
        }
      });
    }

    public IObservable<Contact> GetContact(int id)
    {
      return Observable.FromAsync(async token =>
      {
        var response = await client.GetAsync($"api/contacts/{id}", token);
        if (response.IsSuccessStatusCode)
        {
          return await response.Content.ReadAsAsync<Contact>();
        }
        else
        {
          throw new Exception($"something went wrong when getting contact with id {id}");
        }
      });
    }

    public IObservable<Contact> PostContact(Contact contact)
    {
      return Observable.FromAsync(async token =>
      {
        var response = await client.PostAsJsonAsync("api/contacts", contact, token);
        if (response.IsSuccessStatusCode)
        {
          return await response.Content.ReadAsAsync<Contact>();
        }
        else
        {
          throw new Exception($"something went wrong when posting contact {contact.FirstName} {contact.LastName}");
        }
      });
    }

    public IObservable<Contact> PutContact(Contact contact)
    {
      return Observable.FromAsync(async token =>
      {
        var response = await client.PutAsJsonAsync($"api/contacts/${contact.Id}", contact, token);
        if (response.IsSuccessStatusCode)
        {
          return await response.Content.ReadAsAsync<Contact>();
        }
        else
        {
          throw new Exception($"something went wrong when updating contact with id {contact.Id}");
        }
      });
    }

    public IObservable<int> DeleteContact(int id)
    {
      return Observable.FromAsync(async token =>
      {
        var response = await client.DeleteAsync($"api/contacts/${id}", token);
        if (response.IsSuccessStatusCode)
        {
          return id;
        }
        else
        {
          throw new Exception($"something went wrong when deleting contact with id {id}");
        }
      });
    }

  }
}
