package com.ucd.hyperbuilding;

import astra.core.ActionParam;
import astra.core.Module;
import mams.web.RequestObject;
import mams.web.WebResponse;
import mams.web.WebUtils;

public class Client extends Module {

    @ACTION
    public boolean postRequestAsync(String uri, String body) {
        postRequestAsync(uri, body, "application/json");
        return true;
    }

    void postRequestAsync(String uri, String body, String mediaType) {
        new Thread(() -> sendPostRequest(uri, body)).start();
    }

    private void sendPostRequest(String uri, String body) {
        try {
            RequestObject requestObject = new RequestObject();
            requestObject.method = "POST";
            requestObject.url = uri;
            requestObject.content = body;

            WebResponse response = WebUtils.sendRequest(requestObject);
            System.out.println("Post request async completed, uri=" + uri + ", responseCode=" + response.getCode() +
                    ", responseBody=" + response.getContent());
        } catch (Throwable e) {
            e.printStackTrace();
        }
    }

    @ACTION
    public boolean getRequest(String uri, ActionParam<Integer> responseCode, ActionParam<String> responseString) {
        String defaultMediaType = "application/json";
        WebResponse response = sendGetRequest(uri, defaultMediaType);
        responseCode.set(response.getCode());
        responseString.set(response.getContent());
        return true;
    }

    private WebResponse sendGetRequest(String uri, String mediaType) {
        RequestObject requestObject = new RequestObject();
        requestObject.method = "GET";
        requestObject.url = uri;
        requestObject.type = mediaType;

        return WebUtils.sendRequest(requestObject);
    }

    @ACTION
    public boolean putRequest(String uri, String body, ActionParam<Integer> responseCode, ActionParam<String> responseString) {
        String defaultMediaType = "application/json";
        WebResponse response = sendPutRequest(uri, body, defaultMediaType);
        responseCode.set(response.getCode());
        responseString.set(response.getContent());
        return true;
    }

    private WebResponse sendPutRequest(String uri, String body, String mediaType) {
        RequestObject requestObject = new RequestObject();
        requestObject.method = "PUT";
        requestObject.url = uri;
        requestObject.content = body;
        requestObject.type = mediaType;
        return WebUtils.sendRequest(requestObject);
    }
}