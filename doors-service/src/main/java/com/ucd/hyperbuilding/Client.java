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
        new Thread(() -> {
            try {
                RequestObject requestObject = new RequestObject();
                requestObject.method = "POST";
                requestObject.url = uri;
                requestObject.content = body;
                WebResponse response = WebUtils.sendRequest(requestObject);
                System.out.println("Post request async completed, uri=" + uri + ", responseCode=" + response.getCode() +
                        "responseBod=" + response.getContent());
            } catch (Throwable e) {
                e.printStackTrace();
            }
        }).start();
    }

    @ACTION
    public boolean getRequest(String uri, ActionParam<Integer> responseCode, ActionParam<String> responseString) {
        getRequest(uri, "application/json", responseCode, responseString);
        return true;
    }

    private void getRequest(String uri, String mediaType, ActionParam<Integer> responseCode, ActionParam<String> responseString) {
        RequestObject requestObject = new RequestObject();
        requestObject.method = "GET";
        requestObject.url = uri;
        requestObject.content = null;
        requestObject.type = mediaType;
        WebResponse response = WebUtils.sendRequest(requestObject);
        responseCode.set(response.getCode());
        responseString.set(response.getContent());
    }

    @ACTION
    public boolean putRequest(String uri, String body, ActionParam<Integer> responseCode, ActionParam<String> responseString) {
        putRequest(uri, body, "application/json", responseCode, responseString);
        return true;
    }

    private void putRequest(String uri, String body, String mediaType, ActionParam<Integer> responseCode, ActionParam<String> responseString) {
        RequestObject requestObject = new RequestObject();
        requestObject.method = "PUT";
        requestObject.url = uri;
        requestObject.content = body;
        requestObject.type = mediaType;
        WebResponse response = WebUtils.sendRequest(requestObject);
        responseCode.set(response.getCode());
        responseString.set(response.getContent());
    }
}
